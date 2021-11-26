import { Injectable, Injector } from '@angular/core';
import { Observable, Subscriber } from 'rxjs';

import { ObserversBaseService, OBSERVERS_SERVICE_PART_URL } from 'src/app/observers-base.service';
import { SlotDto } from './slot.dto';


@Injectable()
export class SlotObserversService extends ObserversBaseService {
    private slotSubscriber: Subscriber<SlotDto> | undefined;


    public constructor() {
        const injector = Injector.create({providers: [{provide: OBSERVERS_SERVICE_PART_URL, useValue: 'hubs/slot'}]});
        super(injector.get( OBSERVERS_SERVICE_PART_URL));
    }


    public registerHandlers() {
        this.hubConnection.on('ChangedState', (message) => {
            if (this.slotSubscriber) {
                this.slotSubscriber.next(message)                    
            }
        });
    }   


    public get onChangedState(): Observable<SlotDto> {
        return new Observable((subscriber: Subscriber<SlotDto>) => {
            this.slotSubscriber = subscriber;
        });
    };
}