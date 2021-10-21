using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Train.Models;
using Train.ModelView;
namespace Train.Controllers
{
    public class HomeController : Controller
    {
        Booking_TicketsEntities5 Context = new Booking_TicketsEntities5();
        //Home
        public ActionResult Index()
        {
            return View();
        }
        //Search
        public ActionResult Search()
        {
            return View();
        }
        //SearchResults
        [HttpPost]
        public ActionResult SearchResults(string start, string end, DateTime date, string Train_Class, string Car_Class, int NoOfPass)
        {
            int dir = HelpData.CityId[start] - HelpData.CityId[end];
            List<Train.Models.Train> AllTrains = new List<Models.Train>();
            foreach (Train.Models.Train T in Context.Trains)
            {
                if (Train_Class == "Any" || T.Type == Train_Class)
                {
                    if (((HelpData.CityId[T.StartDestination] - HelpData.CityId[T.EndDestination]) > 0 && dir > 0)
                        ||
                        ((HelpData.CityId[T.StartDestination] - HelpData.CityId[T.EndDestination]) < 0 && dir < 0)
                        )
                    {
                        var cities = T.Goes.Select(G => G.CityName);
                        if (cities.Any(c => c == start) && cities.Any(c => c == end))
                        {
                            AllTrains.Add(T);
                        }
                    }
                }
            }
            var ValidTrains = new List<ValidTrainDetailsModelView>();
            foreach (var train in AllTrains)
            {
                if (HelpData.ValidTrain(train, date, start, end, NoOfPass) != null)
                {
                    ValidTrains.Add(new ValidTrainDetailsModelView
                    {
                        TrainID = train.ID,
                        StartTime = train.Goes.Where(G => G.IDtrain == train.ID && G.CityName == start).FirstOrDefault().Arrive_Time,
                        EndTime = train.Goes.Where(G => G.IDtrain == train.ID && G.CityName == end).FirstOrDefault().Arrive_Time,
                        Price = Math.Abs(HelpData.CityId[end] - HelpData.CityId[start]) * (Car_Class == "First_Class" ? 10 : 5) * (train.Type == "AC" ? 5 : train.Type == "VIP" ? 10 : 15),
                        Type = train.Type,
                        Date = date,
                        train = train
                    });
                }
            }
            Session["Car_Class"] = Car_Class;
            Session["start"] = ViewBag.start = start;
            Session["end"] = ViewBag.end = end;
            Session["NoOfPass"] = ViewBag.NoOfPass = NoOfPass;
            Session["date"] = date;
            return View(ValidTrains.OrderBy(T => T.StartTime).ToList());
        }
        //details
        public ActionResult TrainDetails(int id)
        {

            return View(Context.Goes.Where(G => G.IDtrain == id).OrderBy(G => G.Arrive_Time).ToList());
        }
        //TicketReservation
        public ActionResult TicketReservation(int id)
        {
            Train.Models.Train train = Context.Trains.Where(T => T.ID == id).FirstOrDefault();
            ValidTrainDetailsModelView trainModelView = new ValidTrainDetailsModelView
            {
                StartTime = train.Goes.Where(G => G.IDtrain == train.ID && G.CityName == (string)Session["start"]).FirstOrDefault().Arrive_Time,
                EndTime = train.Goes.Where(G => G.IDtrain == train.ID && G.CityName == (string)Session["end"]).FirstOrDefault().Arrive_Time,
                Price = Math.Abs(HelpData.CityId[(string)Session["end"]] - HelpData.CityId[(string)Session["start"]]) * ((string)Session["Car_Class"] == "First_Class" ? 10 : 5) * (train.Type == "AC" ? 5 : train.Type == "VIP" ? 10 : 15),
                Type = train.Type,
                Date = (DateTime)Session["date"],
                train = train
            };
            Session["trainModelView"] = trainModelView;
            return View();
        }
        //PrintTicket
        public ActionResult PrintTicket()
        {
            return View(Session["newTickets"]as List<Ticket>);
        }
        //add
        public ActionResult Add(int IDPassenger,string Name)
        {
            if (Context.Customers.Any(C => C.ID == IDPassenger) == false)
            {
                Context.Customers.Add(new Customer { Name = Name, ID = IDPassenger });
                Context.SaveChanges();
            }
            int NoOfPass = (int)Session["NoOfPass"];
            ValidTrainDetailsModelView trainModelView = (ValidTrainDetailsModelView)Session["trainModelView"];
            string StartDestination = (string)Session["start"];
            string EndDestination = (string)Session["end"];
            List<Seat> vlidSeats = HelpData.ValidTrain(trainModelView.train, trainModelView.Date, StartDestination, EndDestination, NoOfPass);
            List<Ticket> newTickets = new List<Ticket>();
            foreach(Seat seat in vlidSeats)
            {
                Ticket newTicket = new Ticket { Date = trainModelView.Date, StartDestination = StartDestination, EndDestination = EndDestination, Price = trainModelView.Price, IDPassenger = IDPassenger, SeatID = seat.ID, IDCar = seat.IDCar, IDTrain = seat.IDTrain/*, Seat = seat, Customer = Context.Customers.Where(C => C.ID == IDPassenger).FirstOrDefault()*/ };
                newTickets.Add(newTicket);
                //Context.Seats.Where(S => S.ID == seat.ID).First().Tickets.Add(newTicket);
                //Context.Customers.Where(C => C.ID == IDPassenger).FirstOrDefault().Tickets.Add(newTicket);
                Context.Tickets.Add(newTicket);
                Context.SaveChanges();
            }
            Session["Name"] = Name;
            Session["newTickets"] = newTickets;
            return RedirectToAction("PrintTicket");
        }
        //Cancelation
        public ActionResult Cancelation()
        {
            return View();
        }
        public ActionResult Delete(int ticketId)
        {
            Ticket deletedTicket = Context.Tickets.Where(T => T.ID == ticketId).FirstOrDefault();
            if (deletedTicket == null)
            {
                Session["TicketCanceled"] = false;
                return RedirectToAction("Cancelation");
            }
            else
            {
                Session["TicketCanceled"] = true;
                Context.Tickets.Remove(deletedTicket);
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}