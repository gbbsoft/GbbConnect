# GbbConnect

Program to connect inverters (eg: Deya) and program GbbVictronWeb.gbbsoft.pl.

To connect with inverters program uses SolarmanV5 protocol. (Loggers serial numers: 17xxxxxxx, 21xxxxxxx or 40xxxxxxx)

To connect with GbbVictronWeb program uses Mqtt server and own protocol (see Wiki).

GbbConnect remarks:
- Data on disk are grouped using number in column "No". So if you want start new inverter with new data then put new No.

## Download

Last version download: [GbbConnect.msi](http://www.gbbsoft.pl/!download/GbbConnect/GbbConnectSetup.msi) [setup.exe](http://www.gbbsoft.pl/!download/GbbConnect/setup.exe)

## Connection to inverter

if first connection failed than program tries to connect every 5 minutes.

Program connects to inverter every 1 minute and downloads statistic.

## Connection to mqtt

If first connection to mqtt failed then program tries to connect every 5 minutes.

Program every minute sends keepalive messave to mqtt. If connected has been lost then every minute program tries to reconect.

