import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { LogDto } from './log.dto';


@Injectable()
export class LogObserversService extends ObserversBaseService {
    private subscriber: Subscriber<LogDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/log'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.subscriber) {
                this.subscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<LogDto> {
        return new Observable((subscriber: Subscriber<LogDto>) => {
            this.subscriber = subscriber;
        });
    };
}