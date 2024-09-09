# TacticalOpsQuery

Tactical Ops Web Api

## Resources

### Server:
GET /server-info?ip=10.10.10.10&port=1234

```json
{
  "hostname": "Server Name",
  "hostport": 0,
  "gameVer": 0,
  "mapTitle": "string",
  "mapName": "string",
  "maxPlayers": 0,
  "numPlayers": 0,
  "gameType": "string",
  "scoreTerrorists": 0,
  "scoreSpecialForces": 0,
  "roundNumber": 0,
  "lastWinningTeam": 0,
  "mutators": "string",
  "timeLimit": 0,
  "friendlyFire": "string",
  "tostVersion": "string",
  "adminName": "string",
  "adminEmail": "string",
  "password": "string"
}
```
### Status:
GET /status?ip=10.10.10.10&port=1234

```json
{
  "serverInfo": {
    "hostname": "Server Name",
    "hostport": 0,
    "gameVer": 0,
    "mapTitle": "string",
    "mapName": "string",
    "maxPlayers": 0,
    "numPlayers": 0,
    "gameType": "string",
    "scoreTerrorists": 0,
    "scoreSpecialForces": 0,
    "roundNumber": 0,
    "lastWinningTeam": 0,
    "mutators": "string",
    "timeLimit": 0,
    "friendlyFire": "string",
    "tostVersion": "string",
    "adminName": "string",
    "adminEmail": "string",
    "password": "string"
  },
  "players": [
    {
      "name": "player_name_0",
      "frags": 0,
      "deaths": 0,
      "score": 0,
      "ping": 0,
      "team": 0,
      "mesh": "string",
      "skin": "string",
      "health": 0
    },
    {
      "name": "player_name_1",
      "frags": 0,
      "deaths": 0,
      "score": 0,
      "ping": 0,
      "team": 0,
      "mesh": "string",
      "skin": "string",
      "health": 0
    }
  ]
}
````

### Players:
GET /players?ip=10.10.10.10&port=1234

```json
{
  "name": "player_name",
  "frags": 0,
  "deaths": 0,
  "score": 0,
  "ping": 0,
  "team": 0,
  "mesh": "string",
  "skin": "string",
  "health": 0
}
```

### Teams:
GET /team-info?ip=10.10.10.10&port=1234

```json
[
  {
    "name": "Terrorists",
    "score": 0,
    "players": [
        {
            "name": "player_name_0",
            "frags": 0,
            "deaths": 0,
            "score": 0,
            "ping": 0,
            "team": 0,
            "mesh": "string",
            "skin": "string",
            "health": 0
        }
    ]
  },
  {
    "name": "Special Forces",
    "score": 2,
    "players": [
        {
            "name": "player_name_1",
            "frags": 0,
            "deaths": 0,
            "score": 0,
            "ping": 0,
            "team": 0,
            "mesh": "string",
            "skin": "string",
            "health": 0
        }
    ]
  }
]
```


