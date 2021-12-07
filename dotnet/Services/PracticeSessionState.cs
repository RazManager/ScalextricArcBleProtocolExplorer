using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ScalextricArcBleProtocolExplorer.Services
{
    public class PracticeSessionCarId
    {
        [Required]
        public byte CarId { get; init; }
        public int? Laps { get; set; }
        public double? BestLapTime { get; set; }
        public uint? BestSpeedTrap { get; set; }
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
        public int? Laps { get; set; }
        public string? FastestLapTime { get; set; }
        public uint? FastestSpeedTrap { get; set; }
        public IEnumerable<PracticeSessionLapDto> LatestLaps { get; set; } = new List<PracticeSessionLapDto>();
    }


    public class PracticeSessionLapDto
    {
        public int Lap { get; set; }
        public string? LapTime { get; set; }
        public uint? SpeedTrap { get; set; }
    }


    public class PracticeSessionState
    {
        private readonly IHubContext<Hubs.PracticeSessionHub, Hubs.IPracticeSessionHub> _hubContext;

        public PracticeSessionState(IHubContext<Hubs.PracticeSessionHub, Hubs.IPracticeSessionHub> hubContext)
        {
            _hubContext = hubContext;

            var carIds = new List<PracticeSessionCarId>();
            for (byte i = 0; i < 6; i++)
            {
                carIds.Add(new PracticeSessionCarId
                {
                    CarId = (byte)(i + 1)
                });
            }
            CarIds = carIds;
        }

        private IEnumerable<PracticeSessionCarId> CarIds { get; set; }

        public IEnumerable<PracticeSessionCarIdDto> MapPracticeSessionCarIds()
        {
            return CarIds.Select(x => MapPracticeSessionCarId(x));
        }

        public PracticeSessionCarIdDto MapPracticeSessionCarId(PracticeSessionCarId practiceSessionCarId)
        {
            return new PracticeSessionCarIdDto
            {
                CarId = practiceSessionCarId.CarId,
                Laps = practiceSessionCarId.Laps,
                FastestLapTime = practiceSessionCarId.BestLapTime.HasValue ? practiceSessionCarId.BestLapTime.Value.ToString("F2", CultureInfo.InvariantCulture) : null,
                FastestSpeedTrap = practiceSessionCarId.BestSpeedTrap,
                LatestLaps = practiceSessionCarId.LatestLaps.OrderByDescending(x => x.Lap).Select(x => new PracticeSessionLapDto
                {
                    Lap = x.Lap,
                    LapTime = x.LapTime.HasValue ? x.LapTime.Value.ToString("F2", CultureInfo.InvariantCulture) : null,
                    SpeedTrap = x.SpeedTrap
                })
            };
        }


        public void Reset()
        {
            var carIds = new List<PracticeSessionCarId>();
            for (byte i = 0; i < 6; i++)
            {
                var practiceSessionCarId = new PracticeSessionCarId
                {
                    CarId = (byte)(i + 1)
                };
                carIds.Add(practiceSessionCarId);
                _hubContext.Clients.All.ChangedState(MapPracticeSessionCarId(practiceSessionCarId));
            }
            CarIds = carIds;
        }

        public async Task SetLapTimeAsync(byte carId, uint? lapTime)
        {
            System.Console.WriteLine($"SetLapTimeAsync carId={carId} lapTime={ lapTime}");

            var practiceSessionCarId = CarIds.SingleOrDefault(x => x.CarId == carId);
            if (practiceSessionCarId is not null)
            {
                if (!practiceSessionCarId.Laps.HasValue || !lapTime.HasValue)
                {
                    practiceSessionCarId.Laps = 0;
                    System.Console.WriteLine($"practiceSessionCarId.Laps set to 0");
                }
                else
                {
                    practiceSessionCarId.Laps++;
                    System.Console.WriteLine($"practiceSessionCarId.Laps={practiceSessionCarId}");
                }

                if (!practiceSessionCarId.BestLapTime.HasValue || practiceSessionCarId.BestLapTime / 100.0 < lapTime / 100.0)
                {
                    practiceSessionCarId.BestLapTime = lapTime / 100.0;
                }

                practiceSessionCarId.LatestLaps.Enqueue(new PracticeSessionLap
                {
                    Lap = practiceSessionCarId.Laps.Value,
                    LapTime = lapTime / 100.0
                });
                while (practiceSessionCarId.LatestLaps.Count > 5)
                {
                    practiceSessionCarId.LatestLaps.Dequeue();
                }
                await _hubContext.Clients.All.ChangedState(MapPracticeSessionCarId(practiceSessionCarId));
            }
        }
    }
}