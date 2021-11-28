import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError, finalize } from 'rxjs/operators';

import { CommonBaseService } from '../../lib/common/common-base.service';
import { CommonBusyService } from '../../lib/common/common-busy.service';

import { ConnectionDto } from './connection/connection.dto';
import { CommandDto } from './command/command.dto';
import { DeviceInformationDto } from './device-information/device-information.dto';
import { ThrottleDto } from './throttle/throttle.dto';
import { SlotDto } from './slot/slot.dto';
import { SystemInformationDto } from './system-information/system-information.dto';


@Injectable()
export class ApiService extends CommonBaseService {
    constructor(private readonly httpClient: HttpClient,
                private readonly busyService: CommonBusyService) {
        super('api');
    }


    public getConnection(): Observable<ConnectionDto> {
        this.busyService.begin(this);
        return this.httpClient.get<ConnectionDto>(`${this.serviceUrl}/connection`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
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


    public getDevice(): Observable<DeviceInformationDto> {
        this.busyService.begin(this);
        return this.httpClient.get<DeviceInformationDto>(`${this.serviceUrl}/device-information`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getThrottle(): Observable<ThrottleDto> {
        this.busyService.begin(this);
        return this.httpClient.get<ThrottleDto>(`${this.serviceUrl}/throttle`)
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
}
