﻿using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWolrd.Models;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddStop(string tripName, string username, Stop newStop)
        {
            var theTrip = getTripByName(tripName, username);
            newStop.Order = theTrip.Stops.Count > 0 ? theTrip.Stops.Max(s => s.Order) + 1 : 0;
            _context.Stops.Add(newStop);
        }

        public void AddTrip(Trip newTrip)
        {
            _context.Add(newTrip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            try
            {
                return _context.Trips.OrderBy(t => t.Name).ToList();
            } catch(Exception ex)
            {
                _logger.LogError("Could not get trips from database", ex);
                return null;
            }
        }

        public IEnumerable<Trip> GetAllTripsWithStops()
        {
            try
            {
                return _context.Trips
                    .Include(t => t.Stops)
                    .OrderBy(t => t.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get trips from database", ex);
                return null;
            }
}

        public Trip getTripByName(string tripName, string username)
        {
            return _context.Trips.Include(t => t.Stops)
                .Where(t => t.Name == tripName && t.UserName == username)
                .FirstOrDefault();
        }

        public IEnumerable<Trip> GetUserTripsWithStops(string name)
        {
            try
            {
                return _context.Trips
                    .Include(t => t.Stops)
                    .OrderBy(t => t.Name)
                    .Where(t => t.UserName == name)
                    .ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError("An error with the trips and the db", ex);
                return null;
            }

        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
