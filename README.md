Bot commands:
```
/start -- subscribe you to receive tickets
/unsubscribe -- unsubscribe
/check -- just check if your bot is succesfullyrunning
```
AppSetting:
```
botId -- id from BotFather
Credential -- headers from request from map in practise test (press on gray circle and analyze request)
StartSearchingDate -- Start date of searching. Example: "2025-02-18"
    DaysToSearch: - how many day from start bot will search a ticket. Example: 14,
OfficeId: service number, you can found that info same as Creadentials but on payload section Example: 61 Apostola
MinutesRequestDelay:  Delay between set of request.  So search will be executed every 30 second when value is 0.5
```
When credential expire, you can change them inside bot:
```
/cookie -- set cookie
/rsftoken -- ser X-RSF-Token 
```
