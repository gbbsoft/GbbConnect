# GbbConnect

Program to connect inverters (eg: Deya) and program GbbVictronWeb.gbbsoft.pl.

To connect with inverters program uses SolarmanV5 protocol.

To connect with GbbVictronWeb program uses Mqtt server and own protocol (see Wiki).

GbbConnect remarks:
- Data on disk are grouped using number in column "No". So if you want start new inverter with new data then put new No.

## Connection to inverter

if first connection failed than program tries to connect every 5 minutes.

Program connects to inverter every 1 minute and downloads statistic.

## Connection to mqtt

If first connection to mqtt failed then program tries to connect every 5 minutes.

Program every minute sends keepalive messave to mqtt. If connected has been lost then every minute program tries to reconect.

