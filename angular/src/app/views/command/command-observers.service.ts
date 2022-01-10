import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { CommandDto } from './command.dto';


@Injectable()
export class CommandObserversService extends ObserversBaseService {
    private subscriber: Subscriber<CommandDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/command'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.subscriber) {
                this.subscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<CommandDto> {
        return new Observable((subscriber: Subscriber<CommandDto>) => {
            this.subscriber = subscriber;
        });
    };
}