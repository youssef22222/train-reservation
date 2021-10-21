using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Train.Models
{
    public static class HelpData
    {
        public static void Swap<myType>(ref myType x, ref myType y)
        {
            myType z = x;
            x = y;
            y = z;
        }
        public static Dictionary<string, int> CityId = new Dictionary<string, int>() {
            { "Alexandreia", 0 },
            {"Tanta",1 },
            {"Banha",2 },
            {"Cairo",3 },
            {"Giza",4 },
            {"BeniSuif",5 },
            {"Elminya", 6},
            {"asyut", 7},
            {"Sohag", 8},
            {"Qena", 9},
            {"Luxer", 10},
            {"Aswan", 11},
        };
        public static List<string> CityName = new List<string>()
        {
            "Alexandreia",
            "Tanta",
            "Banha",
            "Cairo",
            "Giza",
            "BeniSuif",
            "Elminya",
            "asyut",
            "Sohag",
            "Qena",
            "Luxer",
            "Aswan",
        };
        public static List<Seat> ValidTrain(Train train, DateTime date, string start, string end,int numOfSeats)
        {
            List<Seat> validSeats = new List<Seat>();
            foreach (car Car in train.cars)
            {
                foreach (Seat seat in Car.Seats)
                {
                    int strtID , endId;
                    bool[] visit = new bool[12];
                    foreach (Ticket ticket in seat.Tickets)
                    {
                        if (ticket.Date != date) continue;
                        strtID = CityId[ticket.StartDestination]; endId = CityId[ticket.EndDestination];
                        if (strtID > endId)
                        {
                            Swap(ref strtID, ref endId);
                        }
                        for (int i = strtID; i <= endId; i++)
                        {
                            visit[i] = true;
                        }
                    }
                    bool flag = true;
                    strtID = CityId[start]; endId = CityId[end];
                    if (strtID > endId)
                    {
                        Swap(ref strtID, ref endId);
                    }
                    for (int i = CityId[start]; flag && i <= CityId[end]; i++)
                    {
                        if (visit[i]) flag = false;
                    }
                    if (flag)
                    {
                        validSeats.Add(seat);
                        if (validSeats.Count == numOfSeats) return validSeats;
                    }
                }
            }
            return null;
        }
    }
}