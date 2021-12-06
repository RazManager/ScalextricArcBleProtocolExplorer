import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { PracticeSessionCarIdDto } from './practice-session.dto';


@Injectable()
export class PracticeSessionObserversService extends ObserversBaseService {
    private subscriber: Subscriber<PracticeSessionCarIdDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/practice-session-car-id'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.subscriber) {
                this.subscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<PracticeSessionCarIdDto> {
        return new Observable((subscriber: Subscriber<PracticeSessionCarIdDto>) => {
            this.subscriber = subscriber;
        });
    };
}