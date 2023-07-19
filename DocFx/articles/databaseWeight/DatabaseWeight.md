# Weight put on database

Amount of requests
---------------------
Because learning could put a heavy burden on a database, we're collecting multiple cards at once (currently 5). 
The value can be easily modified when traffic increases.

Amount of records
---------------------
### Decks
Because we may be storing a lot of data from many users, implementation assumes 1 copy of deck for the user.
This means that user acquiring public deck doesnt generate new copy of deck, and cant modify it. He can only 
learn from it. \

### RevisionLog
There is no history of user learning deck. In RevisionLog we keep only current state of cards.