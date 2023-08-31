import fastf1
import fastf1.plotting
from matplotlib import pyplot as plt
import pandas as pd
import time



def tyre_label_to_compound(name, year, round):
    if year==2018:
        if name=="HYPERSOFT" or name=="ULTRASOFT": return "C5"
        if name=="SUPERSOFT": return "C4"
        if name=="SOFT": return "C3"
        if name=="MEDIUM": return "C2"
        if name=="HARD": return "C1"
        if name=="SUPERHARD": return "C0"
    if year==2019:
        if [2 ,5, 10, 13, 17, 20].__contains__(round):
            if name=="SOFT": return "C3"
            if name=="MEDIUM": return "C2"
            if name=="HARD": return "C1"
        if [1, 3, 4, 8, 9, 11, 12, 14, 16, 18, 19].__contains__(round):
            if name=="SOFT": return "C4"
            if name=="MEDIUM": return "C3"
            if name=="HARD": return "C2"
        if [6, 7, 15, 21].__contains__(round):
            if name=="SOFT": return "C5"
            if name=="MEDIUM": return "C4"
            if name=="HARD": return "C3"
    if year==2020:
        if [4, 6, 9, 12, 14].__contains__(round):
            if name=="SOFT": return "C3"
            if name=="MEDIUM": return "C2"
            if name=="HARD": return "C1"
        if [1, 2, 3, 5, 7, 8, 11, 13, 15, 16].__contains__(round):
            if name=="SOFT": return "C4"
            if name=="MEDIUM": return "C3"
            if name=="HARD": return "C2"
        if [10, 17].__contains__(round):
            if name=="SOFT": return "C5"
            if name=="MEDIUM": return "C4"
            if name=="HARD": return "C3"
    if year==2021:
        if [3, 4, 10, 13].__contains__(round):
            if name=="SOFT": return "C3"
            if name=="MEDIUM": return "C2"
            if name=="HARD": return "C1"
        if [1, 2, 8, 9, 11, 12, 14, 17, 18, 19, 20, 21].__contains__(round):
            if name=="SOFT": return "C4"
            if name=="MEDIUM": return "C3"
            if name=="HARD": return "C2"
        if [5, 6, 7, 15, 16, 22].__contains__(round):
            if name=="SOFT": return "C5"
            if name=="MEDIUM": return "C4"
            if name=="HARD": return "C3"
    if year==2022:
        if [1, 6, 10, 15, 18].__contains__(round):
            if name=="SOFT": return "C3"
            if name=="MEDIUM": return "C2"
            if name=="HARD": return "C1"
        if [2, 4, 5, 12, 13, 14, 16, 19, 20, 21].__contains__(round):
            if name=="SOFT": return "C4"
            if name=="MEDIUM": return "C3"
            if name=="HARD": return "C2"
        if [7, 8, 9, 11, 17, 22].__contains__(round):
            if name=="SOFT": return "C5"
            if name=="MEDIUM": return "C4"
            if name=="HARD": return "C3"
        if round == 3: #Weirdo Australia setup
            if name=="SOFT": return "C5"
            if name=="MEDIUM": return "C3"
            if name=="HARD": return "C2"
    if year==2023:
        if [1, 7, 10, 13, 16, 17].__contains__(round):
            if name=="SOFT": return "C3"
            if name=="MEDIUM": return "C2"
            if name=="HARD": return "C1"
        if [2, 3, 5, 12].__contains__(round):
            if name=="SOFT": return "C4"
            if name=="MEDIUM": return "C3"
            if name=="HARD": return "C2"
        if [4, 6, 8, 9, 11, 14, 15].__contains__(round):
            if name=="SOFT": return "C5"
            if name=="MEDIUM": return "C4"
            if name=="HARD": return "C3"
    return "UNKNOWN"

def normalize_team_name(name):
    if name == "Alfa Romeo Racing" or name == "Alfa Romeo": return "Sauber"
    if name == "AlphaTauri" or name == "Toro Rosso": return "Faenza"
    if name == "Racing Point" or name == "Aston Martin": return "Silverstone"
    if name == "Alpine" or name == "Renault": return "Enstone"
    return name

def plot_stints(drivers, stints):

    # for the plotting to work, you'll need the abbreviations of the drivers from session
    #drivers = session.drivers
    #drivers = [session.get_driver(driver)["Abbreviation"]  for driver in drivers]

    # add plotting for the other coloured tyres

    fig, ax = plt.subplots(figsize=(5, 10))

    for driver in drivers:
        driver_stints = stints.loc[stints["Driver"] == driver]

        previous_stint_end = 0
        for idx, row in driver_stints.iterrows():
            # each row contains the compound name and stint length
            # we can use these information to draw horizontal bars
            plt.barh(
                y=driver,
                width=row["StintLength"],
                left=previous_stint_end,
                color=fastf1.plotting.COMPOUND_COLORS[row["Compound"]],
                edgecolor="black",
                fill=True
            )

            previous_stint_end += row["StintLength"]

    # Make the plot more readable and intuitive
    plt.title("2022 Hungarian Grand Prix Strategies")
    plt.xlabel("Lap Number")
    plt.grid(False)
    # invert the y-axis so drivers that finish higher are closer to the top
    ax.invert_yaxis()

    # Plot aesthetics
    ax.spines['top'].set_visible(False)
    ax.spines['right'].set_visible(False)
    ax.spines['left'].set_visible(False)

    plt.tight_layout()
    plt.show()
    

def get_session_stints(session, year, round):
    laps = session.laps
    weather = session.laps.get_weather_data()

    laps = laps.reset_index(drop=True)
    weather = weather.reset_index(drop=True)
    
    # TODO Fix weather to get it from the time of actual lap, and not beginning of the session
    #laps_with_weather = pd.concat([laps, weather.loc[:, ["AirTemp", "TrackTemp", "Pressure"]]], axis=1)
    #stints = laps_with_weather[["Driver", "Stint", "Compound", "AirTemp", "TrackTemp", "Pressure", "LapNumber"]]

    stints = laps[["Driver", "Team", "Compound", "Stint", "LapNumber"]]
    stints["AirTemp"] = weather.loc[1, "AirTemp"]
    stints["TrackTemp"] = weather.loc[1, "TrackTemp"]
    stints["Pressure"] = weather.loc[1, "Pressure"]

    stints["Season"] = year
    stints["Track"] = session.event.Location
    #TODO Add track length

    stints = stints.groupby(["Season", "Track", "Team", "Driver", "Compound", "Stint", "AirTemp", "TrackTemp", "Pressure"])
    stints = stints.count().reset_index()

    stints = stints.rename(columns={"LapNumber": "StintLength"})
    stints = stints.rename(columns={"Compound": "TyreLabel"})
    stints["Compound"] = stints["TyreLabel"].apply(lambda x: tyre_label_to_compound(x, year, round))
    stints["Team"] = stints["Team"].apply(lambda x: normalize_team_name(x)) 

    stints.drop(columns=["TyreLabel", "Stint"], axis=1, inplace=True)
    stints.drop(stints[stints["Compound"] == "UNKNOWN"].index, inplace=True)

    print(stints)
    return stints

start_time = time.time()

start_year = 2018
end_year = 2023
output_data = pd.DataFrame()

for y in range(start_year, end_year+1):
    number_of_rounds = fastf1.get_event_schedule(y, include_testing=False).shape[0]

    #quick hack to handle middle of the season
    if y == 2023: number_of_rounds = 13

    for r in range(1, number_of_rounds+1):
        session = fastf1.get_session(y, r, 'R')
        session.load()

        stints = get_session_stints(session, y, r)
        output_data = pd.concat([output_data, stints], axis=0)

output_data.to_csv("stints.csv", index=False)

end_time = time.time()
print("Time to gather data %5.2fs" % (end_time - start_time))


