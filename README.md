# GbbConnect

Program to connect inverters (eg: Deya) and program [GbbVictronWeb.gbbsoft.pl.](https://gbbvictronweb.gbbsoft.pl/)

To connect with inverters program uses SolarmanV5 protocol. (Loggers serial numers: 17xxxxxxx, 21xxxxxxx or 40xxxxxxx) (maybe also: 27xxxxxxx, 23xxxxxxx)

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

## Setup in [GbbVictronWeb](https://gbbvictronweb.gbbsoft.pl/)

Manual how setup GbbConnect with Deya and GbbVictronWeb: [Manual](https://gbbvictronweb.gbbsoft.pl/Manual?Filters.Id=8)

# GbbConnectConsole

Program on console.

## Download

Last version download: [GbbConnectConsole.zip](http://www.gbbsoft.pl/!download/GbbConnect/GbbConnectConsole.zip)

## Parameters

-? - list of parameters

--dont-wait-for-key -  don't wait for key, but just wait forever

# Docker

Program GbbConnectConsole can be run in docker. File Dockerfile is present in root directory.

## Configuration file

You can use GbbConnect program to create (and test) configuration file (My Documents\Gbb Connect\Parameters.xml). Then file can be move as /root/GbbConnect/Parameters.xml on docker container.
