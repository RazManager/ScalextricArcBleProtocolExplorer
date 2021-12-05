import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { ConnectionDto } from './connection.dto';


@Injectable()
export class ConnectionObserversService extends ObserversBaseService {
    private subscriber: Subscriber<ConnectionDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/connection'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.subscriber) {
                this.subscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<ConnectionDto> {
        return new Observable((subscriber: Subscriber<ConnectionDto>) => {
            this.subscriber = subscriber;
        });
    };
}