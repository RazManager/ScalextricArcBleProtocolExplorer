import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { ApiErrorResponse } from '../../../lib/common/common-base.service';
import { CommonBaseResolver } from '../../../lib/common/common-base-resolver.service';

import { CarIdDto } from './car-id.dto';
import { ApiService } from '../api.service';


@Injectable()
export class CarIdResolver
        extends CommonBaseResolver
        implements Resolve<CarIdDto> {
    constructor(snackBar: MatSnackBar,
                private readonly apiService: ApiService) {
        super(snackBar);
    }

    
    public resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<CarIdDto> {
        return this.apiService.getCarId()
                              .pipe(catchError((err: ApiErrorResponse) => this.onError(err)));
    }
}