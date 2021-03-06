﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MonitorizareVot.Domain.Ong.Builders;
using MonitorizareVot.Domain.Ong.Models;

namespace MonitorizareVot.Domain.Ong.ObserverAggregate
{
    public class ResetDeviceHandler : AsyncRequestHandler<ResetDeviceCommand, int>
    {
        private readonly VoteMonitorContext _voteMonitorContext;
        private readonly ILogger _logger;

        public ResetDeviceHandler(VoteMonitorContext context, ILogger logger)
        {
            _voteMonitorContext = context;
            _logger = logger;
        }

        protected override Task<int> HandleCore(ResetDeviceCommand request)
        {
            try {
                Observer observer = _voteMonitorContext.Observers
                    .Where(o => o.Phone == request.PhoneNumber && 
                                o.IdNgo == request.IdNgo)
                    .First();

                if (observer == null)
                    return Task.FromResult(-1);

                observer.DeviceRegisterDate = null;
                observer.MobileDeviceId = null;
                //observer.Pin = RandomNumberGenerator.Generate(6);

                _voteMonitorContext.Update(observer);
                return _voteMonitorContext.SaveChangesAsync();
            }
            catch(Exception exception)
            {
                _logger.LogError("Exception caught during resetting of Observer device for id " + request.PhoneNumber, exception);
            }

            return Task.FromResult(-1);
        }
    }
}
