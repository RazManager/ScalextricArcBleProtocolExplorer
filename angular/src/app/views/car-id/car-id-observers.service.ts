import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { CarIdDto } from './car-id.dto';


@Injectable()
export class CarIdObserversService extends ObserversBaseService {
    private subscriber: Subscriber<CarIdDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/car-id'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.subscriber) {
                this.subscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<CarIdDto> {
        return new Observable((subscriber: Subscriber<CarIdDto>) => {
            this.subscriber = subscriber;
        });
    };
}