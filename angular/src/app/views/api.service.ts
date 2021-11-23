import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';

import { CommonBaseService } from '../../lib/common/common-base.service';
import { CommonBusyService } from '../../lib/common/common-busy.service';

import { SystemDto } from './system/system.dto';


@Injectable()
export class ApiService extends CommonBaseService {
    constructor(private readonly httpClient: HttpClient,
                private readonly busyService: CommonBusyService) {
        super('api');
    }


    public getSystem(): Observable<SystemDto> {
        this.busyService.begin(this);
        return this.httpClient.get<SystemDto>(`${this.serviceUrl}/system`)
                              .pipe(catchError((err: HttpErrorResponse) => throwError(() => this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }
}
