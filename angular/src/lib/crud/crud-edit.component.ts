import { Component, OnInit, ChangeDetectorRef, ViewChild, ElementRef, AfterViewInit, InjectionToken, Inject } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { Location } from '@angular/common';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { ApiErrorResponse } from '../common/common-base.service';
import { CommonBaseComponent } from '../common/common-base.component';
import { CommonConfirmComponent } from '../components/common-confirm/common-confirm.component';
import { CommonBreakpointObserver } from '../components/common-breakpointobserver/common-breakpointobserver.service';
import { iCrudDto } from './crud.interface';
import { CrudEditService } from './crud-edit.service';


export const HEADER_NAME = new InjectionToken<string>('HeaderName');


@Component({ template: '' })
export abstract class CrudEditComponent<TReadDto extends iCrudDto, TCreateUpdateDto>
        extends CommonBaseComponent
        implements OnInit, AfterViewInit {
    public formGroup!: FormGroup;
    public add!: boolean;
    public dirty = false;
    public dto!: TReadDto;
    protected eTag!: string;

    @ViewChild('first', {static: false}) firstElementRef! : ElementRef;


    constructor(snackBar: MatSnackBar,
                protected readonly route: ActivatedRoute,
                protected readonly location: Location,
                protected readonly dialog: MatDialog,               
                private readonly changeDetectorRef: ChangeDetectorRef,
                protected readonly commonBreakpointObserver: CommonBreakpointObserver,
                protected readonly entityService: CrudEditService<TReadDto, TCreateUpdateDto>,
                @Inject(HEADER_NAME) private readonly headerName: string) {
        super(snackBar);
    }


    public ngOnInit(): void {
        this.route.data.subscribe({
            next: (data: Data) => {
                this.fromGroupInit(data.result);
            }
        });
    }

    
    protected fromGroupInit([dto, eTag, add]: [TReadDto, string, boolean]): void {
        this.dto = dto;
        this.eTag = eTag;
        this.add = add;
        this.formGroup.patchValue(this.dto);
    }


    public ngAfterViewInit(): void {
//         <mat-form-field>
//     <mat-select formControlName="xyz" cdkFocusInitial>
//         <mat-option value="abc">Abc</mat-option>
//     </mat-select>
// </mat-form-field>
//         Caveat: cdkFocusInitial doesn't do anything on its own. It will only work inside an element with the cdkTrapFocus directive on it. – j2L4e Feb 22 at 20:30 

// console.log('this.firstElementRef', this.firstElementRef);
        if (this.firstElementRef && window.matchMedia('(pointer: fine)').matches) {
            if (this.firstElementRef.nativeElement) {
                this.firstElementRef.nativeElement.focus();
                this.changeDetectorRef.detectChanges();   
            }
        }
    }


    public saveEntity(): void {
        const createUpdateDto = <TCreateUpdateDto><any>{...this.dto};
        Object.assign(createUpdateDto, this.formGroup.value)

        this.onSaveEntity(createUpdateDto);
        if (this.add) {
            this.entityService.createEntity(createUpdateDto)
                                .subscribe({
                                    next: (res: [TReadDto, string]) => this.onEntityCreateUpdateComplete(res[0].id),
                                    error: (err: ApiErrorResponse) => this.onError(err)
                                });
        } else {
            this.entityService.updateEntity(this.dto.id, createUpdateDto, this.eTag)
                                .subscribe({
                                    next: () => this.onEntityCreateUpdateComplete(this.dto.id),
                                    error: (err: ApiErrorResponse) => this.onError(err)
                                });
        }
    }


    protected onSaveEntity(dto: TCreateUpdateDto): void {}


    public deleteEntity(data: string): void {
        const dialogRef = this.dialog.open(CommonConfirmComponent,
                                           { data: data });
        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.entityService.deleteEntity(this.dto.id, this.eTag)
                                .subscribe({
                                    next: () => this.onEntityDeleteComplete(),
                                    error: (error: ApiErrorResponse) => this.onError(error)
                                });
            }
        });
    }


    protected onEntityCreateUpdateComplete(id: string): void {
        this.location.back();
    }


    protected onEntityDeleteComplete(): void {
        this.location.back();
    }


    public cancel(): void {
        this.location.back();
    }


    public get header(): string {
        if (!this.headerId || this.add || this.commonBreakpointObserver.isNarrow) {
            return this.headerName;
        }
        else {
            return this.headerName + " - " +  this.headerId;
        }
    }


    protected abstract get headerId(): string | null;


    public get created(): string {
        return (this.localeString(this.dto.createdAt) + " " +  this.dto.createdUserName).trim();
    }

    
    public get updated(): string {
        return (this.localeString(this.dto.updatedAt) + " " +  this.dto.updatedUserName).trim();
    }
}