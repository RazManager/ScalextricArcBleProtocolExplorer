import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { TrackDto } from './track.dto';


@Injectable()
export class TrackObserversService extends ObserversBaseService {
    private subscriber: Subscriber<TrackDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/track'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.subscriber) {
                this.subscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<TrackDto> {
        return new Observable((subscriber: Subscriber<TrackDto>) => {
            this.subscriber = subscriber;
        });
    };
}