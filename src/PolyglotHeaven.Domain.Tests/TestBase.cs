﻿using System;
using System.Collections.Generic;
using System.Linq;
using PolyglotHeaven.Domain;
using PolyglotHeaven.Infrastructure;
using NUnit.Framework;

namespace PolyglotHeaven.Tests
{
    public class TestBase
    {
        private InMemoryDomainRespository _domainRepository;
        private DomainEntry _domainEntry;
        private Dictionary<Guid, IEnumerable<IEvent>> _preConditions = new Dictionary<Guid, IEnumerable<IEvent>>();

        private DomainEntry BuildApplication()
        {
            _domainRepository = new InMemoryDomainRespository();
            _domainRepository.AddEvents(_preConditions);
            return new DomainEntry(_domainRepository);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            IdGenerator.GenerateGuid = null;
            _preConditions = new Dictionary<Guid, IEnumerable<IEvent>>();
        }

        protected void When<TCommand>(TCommand command) where TCommand : ICommand
        {
            var application = BuildApplication();
            application.ExecuteCommand(command);
        }

        protected void Then(params IEvent[] expectedEvents)
        {
            var latestEvents = _domainRepository.GetLatestEvents().ToList();
            var expectedEventsList = expectedEvents.ToList();
            Assert.AreEqual(expectedEventsList.Count, latestEvents.Count);

            for (int i = 0; i < latestEvents.Count; i++)
            {
                Assert.AreEqual(expectedEvents[i], latestEvents[i]);
            }
        }

        protected void WhenThrows<TException, TCommand>(TCommand command) 
            where TException : Exception 
            where TCommand : ICommand
        {
            try
            {
                When(command);
                Assert.Fail("Expected exception " + typeof(TException));
            }
            catch (TException)
            {
            }
        }

        protected void Given(params IEvent[] existingEvents)
        {
            _preConditions = existingEvents
                .GroupBy(y => y.Id)
                .ToDictionary(y => y.Key, y => y.AsEnumerable());
        }
    }
}
