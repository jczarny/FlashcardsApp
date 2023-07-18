using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FlashcardsApp.Models
{
    public class LearnModel : ILearnModel
    {
        // Depending on stage the card is in (which depends on previous user's results)
        // evaluate when should be the card revised (in days)
        private static readonly Dictionary<int, int> Stage_DaysToRevise = new()
        {
            {0, 4},
            {1, 7},
            {2, 14},
            {3, 30},
            {4, 90},
            {5, 180},
            {6, 360},
        };

        // Depending on an answer given by user, decrease time to next revisision
        // 1 = Again, 2 = Hard, 3 = Medium, 4 = Easy
        private static readonly Dictionary<int, float> Ease_ReviseMulitplier = new Dictionary<int, float>
        {
            {1, 0},
            {2, 0.25f},
            {3, 0.5f},
            {4, 1}
        };

        public static string? _connectionString { get; set; }

        public LearnModel(string connectionString)
        {
            _connectionString = connectionString;
        }

        /*
         *  Gets learning cards by following schema:
         *  Firstly, check if some cards require revision (if date in RevisionLog is past today's one)
         *  Secondly, get new cards that have never been seen (exist in Deck, but not in RevisionLog)
         *  At last, if we run out of cards - signal it to frontend
         */
        public async Task<List<CardDto>> GetLearningCards(int userId, int deckId, int cardAmount)
        {
            // Cards to be returned in a proper order (from more urgent to less)
            List<CardDto> outCards = new();
            // All cards that qualify to be learned now.
            List<Learn> logCards = new();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Sorts in a way that cards, that have their revisionlogs, are on top sorted by a date.
                    // then there is the rest of cards with nulls that dont have logs whatsoever.
                    // Reason for this is to revise old cards first, then learn new ones.
                    SqlCommand cmd = new SqlCommand(
                        $"select * from Cards c left join RevisionLog rl on (c.Id = rl.CardId and rl.UserId={userId})" +
                        $"where (rl.UserId={userId} or rl.UserId is null) and c.DeckId={deckId} " +
                        $"order by case when Date is null then 1 else 0 end, Date asc", connection);

                    await connection.OpenAsync();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Learn logCard = new Learn
                        {
                            CardId = reader.GetInt32("Id"),
                            UserId = reader.IsDBNull("UserId") ? null : reader.GetInt32("UserId"),
                            DeckId = reader.GetInt32("DeckId"),
                            Date = reader.IsDBNull("Date") ? null : reader.GetDateTime("Date"),
                            Stage = reader.IsDBNull("Stage") ? null : reader.GetInt32("Stage"),
                            Front = reader.GetString("Front"),
                            Reverse = reader.GetString("Reverse"),
                            Description = reader.GetString("Description")
                        };
                        logCards.Add(logCard);
                    }
                    reader.Close();

                    foreach (Learn card in logCards)
                    {
                        // If card has no date set, it means it was never seen and is not in RevisionLog
                        // Otherwise it has date describing when should be this card revised again.
                        if (card.Date != null)
                        {
                            // If date today is past the one in database, it means it should be revisioned now
                            if (card.Date <= DateTime.Now)
                            {
                                outCards.Add(new CardDto
                                {
                                    Id = card.CardId,
                                    DeckId = card.DeckId,
                                    Front = card.Front,
                                    Reverse = card.Reverse,
                                    Description = card.Description
                                });
                                if (outCards.Count == cardAmount)
                                    break;
                            }
                        }
                        else
                        {
                            outCards.Add(new CardDto
                            {
                                Id = card.CardId,
                                DeckId = card.DeckId,
                                Front = card.Front,
                                Reverse = card.Reverse,
                                Description = card.Description
                            });
                            if (outCards.Count == cardAmount)
                                break;
                        }
                    }

                    return outCards;
                }
            } catch
            {
                throw;
            }
        }

        /*
         * Evaluate user's knowledge depending on its response now and historical responses.
         * Returns 1 if it evaluates successfully.
         */
        public async Task<int> EvaluateResult(int userId, int cardId, int deckId, int response)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Get previous revision result
                    SqlCommand cmd = new SqlCommand(
                        $"select Ease, Stage from RevisionLog where userId={userId} and cardId={cardId}", connection);

                    await connection.OpenAsync();
                    SqlDataReader reader = cmd.ExecuteReader();

                    // If its user's first encounter with this card - its 'ease' and 'stage' gonna be set on 0.
                    int ease = 1;
                    int stage = 0;
                    bool inRevisionLog = false;

                    if (reader.Read() && !reader.IsDBNull("Ease") && !reader.IsDBNull("Stage"))
                    {
                        inRevisionLog = true;
                        ease = reader.GetInt32("Ease");
                        stage = reader.GetInt32("Stage");
                    }
                    reader.Close();

                    // Calculate values for new/updated revision record
                    DateTime newDate = this.CalcualteNextRevisionDate(stage, response);
                    int newStage = this.CalculateNewStage(stage, response);

                    // If this card was already in revisionLog,
                    // update its values, as past information is no longer necessary.
                    if (inRevisionLog)
                    {
                        cmd = new SqlCommand(
                            $"update RevisionLog set " +
                            $"Date=DATETRUNC(day, @date), Ease={response}, Stage={newStage} " +
                            $"where UserId={userId} and DeckId={deckId} and CardId={cardId}", connection);
                    }
                    // If this card is not in revisionLog then we need to create new record 
                    // As its user's first experience with this card.
                    else
                    {
                        cmd = new SqlCommand(
                            $"insert into RevisionLog (UserId, DeckId, CardId, Date, Ease, Stage) " +
                            $"values ({userId}, {deckId}, {cardId}, DATETRUNC(day, @date), {response}, {newStage})", connection);
                    }

                    cmd.Parameters.Add("@date", SqlDbType.DateTime2).Value = newDate;
                    reader = cmd.ExecuteReader();
                    reader.Close();

                    return 1;
                }
            }
            catch
            {
                throw;
            }
        }
        /*
         * Get amount of cards that need to be revised today.
         * Shown in home's deck card.
         * Return dictionary of deckId and amount of cards to revise for this particular user
         */
        public async Task<Dictionary<int, int>> GetReviseCardAmount(int userId)
        {
            Dictionary<int, int> CardsToReviseByDeck= new();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        $"select DeckId, count(*) as Amount from revisionLog where UserId={userId} and Date<=DATETRUNC(day, GETDATE()) group by DeckId", connection);

                    await connection.OpenAsync();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int deckId = reader.GetInt32("DeckId");
                        int cardsAmount = reader.GetInt32("Amount");
                        CardsToReviseByDeck.Add(deckId, cardsAmount);
                    }
                    return CardsToReviseByDeck;
                }
            } catch
            {
                throw;
            }
        }

        /*
         * Schema for determining next revision date of a card, based on stage and current response.
         */
        private DateTime CalcualteNextRevisionDate(int stage, int response)
        {
            if (response == 1)
                return DateTime.Now.AddMinutes(1);
            else
            {
                int days = (int)Math.Round(Stage_DaysToRevise[stage] * Ease_ReviseMulitplier[response]);
                return DateTime.Now.AddDays(days);
            }
        }
        /*
         * Calculate new stage, depending on current stage (user's fluency of this card) and current response.
         */
        private int CalculateNewStage(int stage, int response)
        {
            if (response == 0)
                return stage - 1;
            else
                return stage + 1;
        }

    }
}
