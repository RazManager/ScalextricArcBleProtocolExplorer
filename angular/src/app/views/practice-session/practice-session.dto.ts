export class PracticeSessionCarIdDto {
    public carId!: number;
    public laps!: number | null;
    public bestLapTime!: number | null;
    public bestSpeedTrap!: number | null;
    public latestLaps!: PracticeSessionLapDto[]
}


export class PracticeSessionLapDto {
    public lap!: number;
    public lapTime!: number | null;
    public speedTrap!: number | null;
}