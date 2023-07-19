# Authentication

Tokens
---------------------
Authentication is provided by implementing access and refresh tokens. \

### Access Tokens
Access tokens are created and passed in body of request. Then, theyre stored in react state. 

### Refresh Tokens
Refresh tokens are created and stored client-side as http-only cookie. Theyre also stored
in database in users table, alongside with their creation and expiration date. \
App implements refresh token rotation, as with access tokens new refresh tokens are generated every user request

On page refresh
---------------------
Refresh tokens are stored in http-only cookie which allows for silent login.