import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { ThrottleProfileDto } from './throttle-profile.dto';


@Injectable()
export class ThrottleProfileObserversService extends ObserversBaseService {
    private subscriber: Subscriber<ThrottleProfileDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/throttle-profile'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.subscriber) {
                this.subscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<ThrottleProfileDto> {
        return new Observable((subscriber: Subscriber<ThrottleProfileDto>) => {
            this.subscriber = subscriber;
        });
    };
}