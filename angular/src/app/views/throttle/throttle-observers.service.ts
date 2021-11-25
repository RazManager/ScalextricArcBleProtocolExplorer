import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { ThrottleDto } from './throttle.dto';


@Injectable()
export class ThrottleObserversService extends ObserversBaseService {
    private throttleSubscriber: Subscriber<ThrottleDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/throttle'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.throttleSubscriber) {
                this.throttleSubscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<ThrottleDto> {
        return new Observable((subscriber: Subscriber<ThrottleDto>) => {
            this.throttleSubscriber = subscriber;
        });
    };
}