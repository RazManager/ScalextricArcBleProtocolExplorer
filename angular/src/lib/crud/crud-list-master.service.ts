import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError, finalize } from 'rxjs/operators';

import { CommonBaseService } from '../common/common-base.service';
import { CommonBusyService } from '../common/common-busy.service';


export abstract class CrudListMasterService<TListDto, TSelectDto> extends CommonBaseService {
    constructor(protected readonly httpClient: HttpClient,
                servicePartUrl: string,
                protected readonly busyService: CommonBusyService) {
        super(servicePartUrl);
    }


    public readEntities(limitFirst: number, limitFollowing: number, offset: number): Observable<TListDto[]> {
        this.busyService.begin(this);
        let params = new HttpParams()
        if (offset === 0) {
            params = params.set('limit', limitFirst.toString());
        }
        else {
            params = params.set('limit', limitFollowing.toString());
        }
        params = params.append('offset', offset.toString());
        return this.httpClient.get<TListDto[]>(this.serviceUrl,
                                        {
                                            params: params
                                        })
                               .pipe(catchError((err: HttpErrorResponse) => throwError(this.getApiError(err))),
                                     finalize(() => this.busyService.end(this)));
    }


    public readEntitiesSelect(idAlwaysInclude: string[]): Observable<TSelectDto[]> {
        let params = new HttpParams();
        if (idAlwaysInclude) {
            idAlwaysInclude.forEach(id => {
                params = params.append('idAlwaysInclude', id);
            });
        }
        this.busyService.begin(this);
        return this.httpClient.get<TSelectDto[]>(`${this.serviceUrl}/select`,
                                            {
                                                params: params
                                            })
                              .pipe(catchError((err: HttpErrorResponse) => throwError(this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }
}
