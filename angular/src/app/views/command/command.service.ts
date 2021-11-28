import { Injectable } from '@angular/core';

import { CommandDto } from './command.dto';


@Injectable()
export class CommandService {
    public dto!: CommandDto;
}
