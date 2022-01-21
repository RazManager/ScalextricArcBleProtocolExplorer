export enum CommandType {
    NoPowerTimerStopped = "NoPowerTimerStopped",
    NoPowerTimerTicking = "NoPowerTimerTicking",
    PowerOnRaceTrigger = "PowerOnRaceTrigger",
    PowerOnRacing = "PowerOnRacing",
    PowerOnTimerHalt = "PowerOnTimerHalt",
    NoPowerRebootPic18 = "NoPowerRebootPic18"
}

export class CommandDto {
    public command!: CommandType;
    public powerMultiplier1!: number;
    public powerBitSix1!: boolean;
    public ghost1!: boolean;
    public rumble1!: number;
    public brake1!: number;
    public kers1!: boolean;
    public powerMultiplier2!: number;
    public powerBitSix2!: boolean;
    public ghost2!: boolean;
    public rumble2!: number;
    public brake2!: number;
    public kers2!: boolean;
    public powerMultiplier3!: number;
    public powerBitSix3!: boolean;
    public ghost3!: boolean;
    public rumble3!: number;
    public brake3!: number;
    public kers3!: boolean;
    public powerMultiplier4!: number;
    public powerBitSix4!: boolean;
    public ghost4!: boolean;
    public rumble4!: number;
    public brake4!: number;
    public kers4!: boolean;
    public powerMultiplier5!: number;
    public powerBitSix5!: boolean;
    public ghost5!: boolean;
    public rumble5!: number;
    public brake5!: number;
    public kers5!: boolean;
    public powerMultiplier6!: number;
    public powerBitSix6!: boolean;
    public ghost6!: boolean;
    public rumble6!: number;
    public brake6!: number;
    public kers6!: boolean;
}