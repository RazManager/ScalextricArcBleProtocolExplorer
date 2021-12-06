import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError, finalize } from 'rxjs/operators';

import { CommonBaseService } from '../../lib/common/common-base.service';
import { CommonBusyService } from '../../lib/common/common-busy.service';

import { CarIdDto } from './car-id/car-id.dto';
import { CommandDto } from './command/command.dto';
import { ConnectionDto } from './connection/connection.dto';
import { GattCharacteristicDto } from './gatt-characteristic/gatt-characteristic.dto';
import { LogDto } from './log/log.dto';
import { PracticeSessionCarIdDto } from './practice-session/practice-session.dto';
import { SlotDto } from './slot/slot.dto';
import { SystemInformationDto } from './system-information/system-information.dto';
import { ThrottleDto } from './throttle/throttle.dto';
import { ThrottleProfileDto } from './throttle-profile/throttle-profile.dto';
import { TrackDto } from './track/track.dto';


@Injectable()
export class ApiService extends CommonBaseService {
    constructor(private readonly httpClient: HttpClient,
                private readonly busyService: CommonBusyService) {
        super('api');
    }


    public getCarId(): Observable<CarIdDto> {
        this.busyService.begin(this);
        return this.httpClient.get<CarIdDto>(`${this.serviceUrl}/car-id`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public postCarId(dto: CarIdDto): Observable<void> {
        this.busyService.begin(this);
        return this.httpClient.post(`${this.serviceUrl}/car-id`, dto)
                              .pipe(map(() => {}),
                                    catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getCommand(): Observable<CommandDto> {
        this.busyService.begin(this);
        return this.httpClient.get<CommandDto>(`${this.serviceUrl}/command`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public postCommand(dto: CommandDto): Observable<void> {
        this.busyService.begin(this);
        return this.httpClient.post(`${this.serviceUrl}/command`, dto)
                              .pipe(map(() => {}),
                                    catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getConnection(): Observable<ConnectionDto> {
        this.busyService.begin(this);
        return this.httpClient.get<ConnectionDto>(`${this.serviceUrl}/connection`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getGattCharacteristics(): Observable<GattCharacteristicDto[]> {
        this.busyService.begin(this);
        return this.httpClient.get<GattCharacteristicDto[]>(`${this.serviceUrl}/gatt-characteristics`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getLogs(): Observable<LogDto[]> {
        this.busyService.begin(this);
        return this.httpClient.get<LogDto[]>(`${this.serviceUrl}/logs`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getPracticeSessionCarIds(): Observable<PracticeSessionCarIdDto[]> {
        this.busyService.begin(this);
        return this.httpClient.get<PracticeSessionCarIdDto[]>(`${this.serviceUrl}/practice-session-car-ids`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }
    
    
    public getSlots(): Observable<SlotDto[]> {
        this.busyService.begin(this);
        return this.httpClient.get<SlotDto[]>(`${this.serviceUrl}/slots`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getSystem(): Observable<SystemInformationDto> {
        this.busyService.begin(this);
        return this.httpClient.get<SystemInformationDto>(`${this.serviceUrl}/system-information`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getThrottle(): Observable<ThrottleDto> {
        this.busyService.begin(this);
        return this.httpClient.get<ThrottleDto>(`${this.serviceUrl}/throttle`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getThrottleProfiles(): Observable<ThrottleProfileDto[]> {
        this.busyService.begin(this);
        return this.httpClient.get<ThrottleProfileDto[]>(`${this.serviceUrl}/throttle-profiles`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public postThrottleProfile(dto: ThrottleProfileDto): Observable<void> {
        this.busyService.begin(this);
        return this.httpClient.post(`${this.serviceUrl}/throttle-profiles`, dto)
                              .pipe(map(() => {}),
                                    catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getTrack(): Observable<TrackDto> {
        this.busyService.begin(this);
        return this.httpClient.get<TrackDto>(`${this.serviceUrl}/track`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }
}