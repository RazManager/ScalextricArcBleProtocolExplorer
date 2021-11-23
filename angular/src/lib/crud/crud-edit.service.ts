import { HttpClient, HttpHeaders, HttpResponse, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError, finalize } from 'rxjs/operators';

import { CommonBaseService } from '../common/common-base.service';
import { CommonBusyService } from '../common/common-busy.service';
import { Params } from '@angular/router';


export abstract class CrudEditService<TReadDto, TCreateUpdateDto>
        extends CommonBaseService {
    public addedId: string | null = null;


    constructor(protected readonly httpClient: HttpClient,
                servicePartUrl: string,
                protected readonly busyService: CommonBusyService) {
        super(servicePartUrl);
    }


    public readDefaultEntity(queryParameters: Params): Observable<TReadDto> {
        let params = new HttpParams();
        if (queryParameters) {           
            Object.keys(queryParameters).forEach(key => {
                params = params.append(key, queryParameters[key]);
            });
        }

        this.busyService.begin(this);
        return this.httpClient.get<TReadDto>(`${this.serviceUrl}/default`,
                    {
                        params: params
                    })
                              .pipe(catchError((err: HttpErrorResponse) => throwError(this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public readEntity(id: string): Observable<[TReadDto, string]> {
        this.busyService.begin(this);
        return this.httpClient.get<TReadDto>(`${this.serviceUrl}/${id}`,
                                            {observe: 'response'})
                              .pipe(map(res => this.extractEntity(res)),
                                    catchError((err: HttpErrorResponse) => throwError(this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public createEntity(dto: TCreateUpdateDto): Observable<[TReadDto, string]> {
        this.busyService.begin(this);
        return this.httpClient.post<TReadDto>(this.serviceUrl,
                                             dto,
                                             {observe: 'response'})
                              .pipe(map(res => this.extractEntity(res)),
                                    catchError((err: HttpErrorResponse) => throwError(this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public updateEntity(id: string, dto: TCreateUpdateDto, eTag: string): Observable<void> {
        this.busyService.begin(this);
        return this.httpClient.put(`${this.serviceUrl}/${id}`,
                                   dto,
                                   { headers: new HttpHeaders({'If-Match': eTag}) })
                              .pipe(map(() => {}),
                                    catchError((err: HttpErrorResponse) => throwError(this.getApiError(err))),
                                    finalize(() => this.busyService.end(this)));
    }


    public deleteEntity(id: string, eTag: string): Observable<void> {
        this.busyService.begin(this);
        return this.httpClient.delete(`${this.serviceUrl}/${id}`,
                                      { headers: new HttpHeaders({'If-Match': eTag}) })
                               .pipe(map(() => {}),
                                     catchError((err: HttpErrorResponse) => throwError(this.getApiError(err))),
                                     finalize(() => this.busyService.end(this)));
    }


    protected extractEntity(res: HttpResponse<TReadDto>): [TReadDto, string] {
        return [res.body!, res.headers.get('ETag')!];
    }
}
