export class PracticeSessionCarIdDto {
    public carId!: number;
    public controllerOn!: boolean;
    public laps!: number | null;
    public fastestLapTime!: string | null;
    public fastestSpeedTrap!: number | null;
    public analogPitstop!: boolean;
    public latestLaps!: PracticeSessionLapDto[]
}


export class PracticeSessionLapDto {
    public lap!: number;
    public lapTime!: string | null;
    public speedTrap!: number | null;
}