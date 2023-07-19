# Learning process

Our learning process is very much anki-like, but simplified. \

## Revising card
Depending on user's response on the card, different revision date will be set.

Revision date is calculated as follows:
- If user failed to remember the card: Set revision date equal to 1 minute from now
- Otherwise, use a formula: $Stage * EaseMultiplier$ \
where stage is his remembered profficiency in this card(previous responses), and ease is his current response. \
Defined values can be found in Models/LearnModel.cs

## Gathering cards
Cards are provided to user in following order:
- First, we're collecting cards we ought to revise most urgently (Date is most past),
- When we run out of these, then we're using new cards never seen by user.