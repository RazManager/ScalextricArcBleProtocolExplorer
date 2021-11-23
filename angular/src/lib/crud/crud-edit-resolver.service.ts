import { Router, Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';

import { iCrudDto } from './crud.interface';
import { CrudEditService } from './crud-edit.service';
import { CrudEditRead } from './crud-edit-get.service';


export abstract class CrudEditResolver<TReadDto extends iCrudDto, TCreateUpdateDto>
        extends CrudEditRead<TReadDto, TCreateUpdateDto>
        implements Resolve<[TReadDto, string | null, boolean]> {

    constructor(snackBar: MatSnackBar,
                router: Router,                
                entityService: CrudEditService<TReadDto, TCreateUpdateDto>) {
        super(snackBar, router, entityService);
    }


    public resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<[TReadDto, string | null, boolean]> {
        return this.get(route);
    }
}
