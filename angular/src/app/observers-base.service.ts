import { Injectable, OnDestroy, InjectionToken, Inject } from '@angular/core';
import * as signalR from "@microsoft/signalr";

import { environment } from 'src/environments/environment';


export const OBSERVERS_SERVICE_PART_URL = new InjectionToken<string>('ObserversServicePartUrl');


@Injectable()
export abstract class ObserversBaseService implements OnDestroy {   
    protected readonly hubConnection: signalR.HubConnection;


    constructor(@Inject(OBSERVERS_SERVICE_PART_URL) servicePartUrl: string) {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(
                environment.apiBaseUrl + servicePartUrl,
            )
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Trace)
            .build();    
    }


    public ngOnDestroy(): void {
        this.hubConnection.stop();
    }


    public observe(): void {
        this.registerHandlers();
        this.hubConnection
            .start()
            .then(() => {
                this.hubConnection.invoke("Observe")
                    .catch(err => console.error('Error while invoking: ' + err));
            })
            .catch(err => console.error('Error while starting connection: ' + err))        
    }


    public abstract registerHandlers(): void;
}