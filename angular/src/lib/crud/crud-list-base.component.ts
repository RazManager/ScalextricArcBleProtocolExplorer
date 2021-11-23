import { NgZone } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { ScrollDispatcher, CdkScrollable } from '@angular/cdk/overlay';
import { map, distinctUntilChanged } from 'rxjs/operators';

import { CommonBaseService } from '../common/common-base.service';
import { CommonBaseComponent } from '../common/common-base.component';


export abstract class CrudListBaseComponent<TListDto>
        extends CommonBaseComponent {
    protected limitFirst: number = 30;
    protected limitFollowing: number = 10;
    public items = new Array<TListDto>();
    public dataSource: MatTableDataSource<TListDto> = new MatTableDataSource<TListDto>(this.items);


    constructor(snackBar: MatSnackBar,
                private readonly scrollDispatcher: ScrollDispatcher,
                private readonly ngZone: NgZone,
                protected readonly entityService: CommonBaseService) {
        super(snackBar);

        this.scrollDispatcher
            .scrolled()
            .pipe(map((data: void | CdkScrollable) => {
                        if (!data) {
                            return false;
                        }
                        const element = data.getElementRef().nativeElement;
                        return element.offsetHeight + element.scrollTop >= element.scrollHeight - 200;
                    }),
                distinctUntilChanged())
            .subscribe((more: boolean) => {
                if (more) {
                    this.ngZone.run(() => {
                        this.getEntities();
                    });                             
                }
            });
    }


    protected abstract getEntities(): void;


    protected reset(): void {
        this.items = new Array<TListDto>();    
    }


    protected onEntitiesRetrieved(res: TListDto[]): void {
        res.forEach(element => {
            this.items.push(element);
        });
        this.dataSource = new MatTableDataSource<TListDto>(this.items);
    }
}
