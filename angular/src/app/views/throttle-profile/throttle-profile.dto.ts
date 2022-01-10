export class ThrottleProfileDto {
    public carId!: number;
    public values!: ThrottleProfileValueDto[];
}

export class ThrottleProfileValueDto {
    public value!: number;
}