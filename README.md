## What is Reactivities? 
Reactivities is an API application where users can plan their activities, follow other users and visualise their own activities. This application offers users the possibility to organise, share and track their activities. In addition, users can review their activities in a visual way and engage in social interaction by following others' activities. Reactivities, is a comprehensive API application that allows users to interact with their activities on a social platform.
Built with: .Net Core WebAPI, Entity Framework Core, PostgreSQL, SignalR

[Report Bug]("https://github.com/berkaykarakush/Reactivities/issues/new")

## Installation
`git clone https://github.com/berkaykarakush/Reactivities.git` or `docker pull berkaykarakush/reactivities`

## How to use this image
`docker run --rm -it -p 8080:80 --env TokenKey='<Your_Token_Key>' --env Facebook:AppId='<Your_API_Key>' --env Facebook:ApiSecret='<Your_API_Secret>' --env ConnectionStrings:DefaultConnection='<PostgreSql_ConnectionString>' --env Cloudinary:CloudName='<Your_CloudName>'--env Cloudinary:ApiSecret='<Your_API_Secret>' --env Cloudinary:ApiKey='<Your_API_Key>' --name reactivities berkaykarakush/reactivities`

## Using
### Account Controller
Login: POST `api/account/login`
```
Request body
{
  "email": "string",
  "password": "string"
}
```
Register: POST `/api/account/register`
```
Request body
{
  "displayName": "string",
  "email": "user@example.com",
  "password": ">[5Z",
  "username": "string"
}
```
Verify Email: POST `/api/Account/verifyEmail?token={token}&email={email}`

Resend Email Confirmation Link: GET `/api/Account/resendEmailConfirmationLink?email={email}`

Get Current User: GET `/api/account`

Facebook Login: POST `/api/accoint/fbLogin?accessToken={accessToken}`

Refresh Token: POST `/api/Account/refreshToken`

### Activities Controller
Activities: GET `/api/activities`

Activity: GET `/api/activities/{id}`

Create: POST `/api/activities`
```
Request body
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "string",
  "date": "2023-12-27T09:58:06.596Z",
  "description": "string",
  "category": "string",
  "city": "string",
  "venue": "string"
}
```

Edit: PUT `/api/activities/{id}`

Delete: DELETE `/api/activities/{id}`

Attend: POST `/api/activities/{id}/attend`

### Buggy Controller
Not Found: GET `/api/buggy/not-found`

Bad Request: GET `/api/buggy/bad-request`

Server Error: GET `/api/buggy/server-error`

Unauthorised: GET `/api/buggy/unauthorised`

### Follow Controller
Follow: POST `/api/follow/{username}`

Get Followings: GET `/api/Follow/{username}`

### Photos Controller
Add: POST `/api/photos/{File}`

Delete: DELETE `/api/photos/{id}`

Set Main: POST `/api/photos/{id}/setmain`

### Profiles Controller 
GetProfile: GET `/api/profiles/{username}`

GetUserActivities: GET `/api/profiles/{username}/activactivitiesitie`