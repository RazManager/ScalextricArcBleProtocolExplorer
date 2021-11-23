import { Injectable, OnDestroy, InjectionToken, Inject } from '@angular/core';
import * as signalR from "@microsoft/signalr";

import { environment } from 'src/environments/environment';
import { IdentityService } from 'src/app/identity/identity.service';


export const OBSERVERS_SERVICE_PART_URL = new InjectionToken<string>('ObserversServicePartUrl');


@Injectable()
export abstract class ObserversBaseService implements OnDestroy {   
    protected readonly hubConnection: signalR.HubConnection;


    constructor(identityService: IdentityService,
                @Inject(OBSERVERS_SERVICE_PART_URL) servicePartUrl: string) {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(
                environment.apiBaseUrl + servicePartUrl,
                { accessTokenFactory: () => identityService.accessToken! }
            )
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Trace)
            .build();    
    }


    public ngOnDestroy(): void {
        this.hubConnection.stop();
    }


    public observe(id: string): void {
        this.registerHandlers();
        this.hubConnection
            .start()
            .then(() => {
                //console.log('Connection started');
                this.hubConnection.invoke("Observe", id)
                    //.then(() => console.log('Invoke finished.'))
                    .catch(err => console.error('Error while invoking: ' + err));
            })
            .catch(err => console.error('Error while starting connection: ' + err))        
    }


    public abstract registerHandlers(): void;
}