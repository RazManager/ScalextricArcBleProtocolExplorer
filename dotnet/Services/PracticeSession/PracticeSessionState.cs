using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Services.PracticeSession
{
    public class PracticeSessionCarId
    {
        public byte CarId { get; init; }
        public bool ControllerOn { get; set; } = false;
        public bool GhostOn { get; set; } = false;
        public bool ControllerOrGhostOn { get; set; } = false;
        public int? Laps { get; set; }
        public double? FastestLapTime { get; set; }
        public uint? FastestSpeedTrap { get; set; }
        public bool AnalogPitstop { get; set; } = false;
        public Queue<PracticeSessionLap> LatestLaps { get; set; } = new Queue<PracticeSessionLap>();
    }


    public class PracticeSessionLap
    {
        public int Lap { get; set; }
        public double? LapTime { get; set; }
        public uint? SpeedTrap { get; set; }
    }


    public class PracticeSessionCarIdDto
    {
        [Required]
        public byte CarId { get; init; }
        [Required]
        public bool ControllerOrGhostOn { get; set; } = false;
        public int? Laps { get; set; }
        public string? FastestLapTime { get; set; }
        public uint? FastestSpeedTrap { get; set; }
        [Required]
        public bool AnalogPitstop { get; set; } = false;
        [Required]
        public IEnumerable<PracticeSessionLapDto> LatestLaps { get; set; } = new List<PracticeSessionLapDto>();
    }


    public class PracticeSessionLapDto
    {
        [Required]
        public int Lap { get; set; }
        public string? LapTime { get; set; }
        public uint? SpeedTrap { get; set; }
    }


    public class PracticeSessionState
    {
        private readonly IHubContext<Hubs.PracticeSessionHub, Hubs.IPracticeSessionHub> _hubContext;
        private readonly ILogger<PracticeSessionState> _logger;

        public PracticeSessionState(IHubContext<Hubs.PracticeSessionHub, Hubs.IPracticeSessionHub> hubContext,
                                    ILogger<PracticeSessionState> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
            CarIds = new List<PracticeSessionCarId>();
            for (byte i = 0; i < 6; i++)
            {
                CarIds.Add(new PracticeSessionCarId
                {
                    CarId = (byte)(i + 1)
                });
            }
        }

        private List<PracticeSessionCarId> CarIds { get; set; }

        public IEnumerable<PracticeSessionCarIdDto> MapPracticeSessionCarIds()
        {
            return CarIds.Select(x => MapPracticeSessionCarId(x));
        }

        public PracticeSessionCarIdDto MapPracticeSessionCarId(PracticeSessionCarId practiceSessionCarId)
        {
            return new PracticeSessionCarIdDto
            {
                CarId = practiceSessionCarId.CarId,
                ControllerOrGhostOn = practiceSessionCarId.ControllerOrGhostOn,
                Laps = practiceSessionCarId.Laps,
                FastestLapTime = practiceSessionCarId.FastestLapTime.HasValue ? (practiceSessionCarId.FastestLapTime.Value / 100.0).ToString("F2", CultureInfo.InvariantCulture) : null,
                FastestSpeedTrap = practiceSessionCarId.FastestSpeedTrap,
                AnalogPitstop = practiceSessionCarId.AnalogPitstop,
                LatestLaps = practiceSessionCarId.LatestLaps.OrderByDescending(x => x.Lap).Select(x => new PracticeSessionLapDto
                {
                    Lap = x.Lap,
                    LapTime = x.LapTime.HasValue ? (x.LapTime.Value / 100.0).ToString("F2", CultureInfo.InvariantCulture) : null,
                    SpeedTrap = x.SpeedTrap
                })
            };
        }

        public async Task ResetAsync()
        {
            CarIds = new List<PracticeSessionCarId>();
            for (byte i = 0; i < 6; i++)
            {
                var practiceSessionCarId = new PracticeSessionCarId
                {
                    CarId = (byte)(i + 1)
                };
                CarIds.Add(practiceSessionCarId);
                await _hubContext.Clients.All.ChangedState(MapPracticeSessionCarId(practiceSessionCarId));
            }
        }

        public Task SetLapTimeAsync(byte carId, uint? lapTime)
        {
            try
            {
                var practiceSessionCarId = CarIds.SingleOrDefault(x => x.CarId == carId);
                if (practiceSessionCarId is not null)
                {
                    practiceSessionCarId.AnalogPitstop = true;

                    if (!practiceSessionCarId.Laps.HasValue || !lapTime.HasValue)
                    {
                        practiceSessionCarId.Laps = 0;
                    }
                    else
                    {
                        practiceSessionCarId.Laps++;
                    }

                    if (!practiceSessionCarId.FastestLapTime.HasValue || practiceSessionCarId.FastestLapTime.Value > lapTime)
                    {
                        practiceSessionCarId.FastestLapTime = lapTime;
                    }

                    practiceSessionCarId.LatestLaps.Enqueue(new PracticeSessionLap
                    {
                        Lap = practiceSessionCarId.Laps.Value,
                        LapTime = lapTime
                    });
                    while (practiceSessionCarId.LatestLaps.Count > 5)
                    {
                        practiceSessionCarId.LatestLaps.Dequeue();
                    }
                    _hubContext.Clients.All.ChangedState(MapPracticeSessionCarId(practiceSessionCarId));
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
            return Task.CompletedTask;
        }

        public Task SetSpeedTrapAsync(byte carId, uint? speedTrap)
        {
            try
            {
                var practiceSessionCarId = CarIds.SingleOrDefault(x => x.CarId == carId);
                if (practiceSessionCarId is not null && practiceSessionCarId.LatestLaps.Count > 0)
                {
                    practiceSessionCarId.AnalogPitstop = false;

                    if (!practiceSessionCarId.FastestSpeedTrap.HasValue || practiceSessionCarId.FastestSpeedTrap > speedTrap)
                    {
                        practiceSessionCarId.FastestSpeedTrap = speedTrap;
                    }

                    practiceSessionCarId.LatestLaps.ElementAt(practiceSessionCarId.LatestLaps.Count - 1).SpeedTrap = speedTrap;
                    _hubContext.Clients.All.ChangedState(MapPracticeSessionCarId(practiceSessionCarId));
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
            return Task.CompletedTask;
        }

        public Task SetCtrlVersionAsync
        (
            byte ctrlVersion1,
            byte ctrlVersion2,
            byte ctrlVersion3,
            byte ctrlVersion4,
            byte ctrlVersion5,
            byte ctrlVersion6,
            bool isDigital
        )
        {
            _ = SetCtrlVersionCarIdAsync(1, ctrlVersion1, isDigital);
            _ = SetCtrlVersionCarIdAsync(2, ctrlVersion2, isDigital);
            _ = SetCtrlVersionCarIdAsync(3, ctrlVersion3, isDigital);
            _ = SetCtrlVersionCarIdAsync(4, ctrlVersion4, isDigital);
            _ = SetCtrlVersionCarIdAsync(5, ctrlVersion5, isDigital);
            _ = SetCtrlVersionCarIdAsync(6, ctrlVersion6, isDigital);
            return Task.CompletedTask;
        }

        private Task SetCtrlVersionCarIdAsync(byte carId, byte ctrlVersion, bool isDigital)
        {
            var practiceSessionCarId = CarIds.SingleOrDefault(x => x.CarId == carId);
            if (practiceSessionCarId is not null)
            {
                practiceSessionCarId.ControllerOn = ctrlVersion < 255 && (carId <= 2 || isDigital);
                SetControllerOrGhostOnCarId(carId);
            }
            return Task.CompletedTask;
        }

        public Task SetGhostAsync
        (
            bool ghost1,
            bool ghost2,
            bool ghost3,
            bool ghost4,
            bool ghost5,
            bool ghost6
        )
        {
            _ = SetGhostCarIdAsync(1, ghost1);
            _ = SetGhostCarIdAsync(2, ghost2);
            _ = SetGhostCarIdAsync(3, ghost3);
            _ = SetGhostCarIdAsync(4, ghost4);
            _ = SetGhostCarIdAsync(5, ghost5);
            _ = SetGhostCarIdAsync(6, ghost6);
            return Task.CompletedTask;
        }

        private Task SetGhostCarIdAsync(byte carId, bool ghost)
        {
            var practiceSessionCarId = CarIds.SingleOrDefault(x => x.CarId == carId);
            if (practiceSessionCarId is not null)
            {
                practiceSessionCarId.GhostOn = ghost;
                SetControllerOrGhostOnCarId(carId);
            }
            return Task.CompletedTask;
        }

        private void SetControllerOrGhostOnCarId(byte carId)
        {
            try
            {
                var practiceSessionCarId = CarIds.SingleOrDefault(x => x.CarId == carId);
                if (practiceSessionCarId is not null)
                {
                    var controllerOrGhostOn = practiceSessionCarId.ControllerOn || practiceSessionCarId.GhostOn;

                    if (practiceSessionCarId.ControllerOrGhostOn != controllerOrGhostOn)
                    {
                        practiceSessionCarId.ControllerOrGhostOn = controllerOrGhostOn;
                        _hubContext.Clients.All.ChangedState(MapPracticeSessionCarId(practiceSessionCarId));
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }
    }
}