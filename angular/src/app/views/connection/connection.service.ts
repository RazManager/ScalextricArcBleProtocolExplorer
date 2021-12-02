import { Injectable } from '@angular/core';

import { ConnectionDto } from './connection.dto';


@Injectable()
export class ConnectionService {
    public dto!: ConnectionDto;
}
