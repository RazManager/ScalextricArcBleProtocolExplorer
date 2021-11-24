import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';

import { CommonBaseService } from '../../lib/common/common-base.service';
import { CommonBusyService } from '../../lib/common/common-busy.service';

import { DeviceDto } from './device/device.dto';
import { ThrottleDto } from './throttle/throttle.dto';
import { SystemDto } from './system/system.dto';


@Injectable()
export class ApiService extends CommonBaseService {
    constructor(private readonly httpClient: HttpClient,
                private readonly busyService: CommonBusyService) {
        super('api');
    }


    public getDevice(): Observable<DeviceDto> {
        this.busyService.begin(this);
        return this.httpClient.get<DeviceDto>(`${this.serviceUrl}/device`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getThrottle(): Observable<ThrottleDto> {
        this.busyService.begin(this);
        return this.httpClient.get<ThrottleDto>(`${this.serviceUrl}/throttle`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public getSystem(): Observable<SystemDto> {
        this.busyService.begin(this);
        return this.httpClient.get<SystemDto>(`${this.serviceUrl}/system`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }
}
