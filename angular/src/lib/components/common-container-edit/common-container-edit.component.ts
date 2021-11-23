import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BreakpointState } from '@angular/cdk/layout';

import { CommonBreakpointObserver } from '../common-breakpointobserver/common-breakpointobserver.service';


@Component({
    selector: 'common-container-edit',
    templateUrl: './common-container-edit.component.html'
})
export class CommonContainerEditComponent
        implements OnInit {
    @Input() formGroup!: FormGroup;
    @Input() header!: string;
    @Input() dirty = false;
    @Input() add!: boolean;
    @Input() crudButtons = true;
    @Output() menuClick: EventEmitter<void> = new EventEmitter<void>();
    @Output() deleteClick: EventEmitter<void> = new EventEmitter<void>();
    @Output() cancelClick: EventEmitter<void> = new EventEmitter<void>();

    public menuToolbarShow = false;
    public saveToolbarShow = true;
    public deleteToolbarShow = true;
    public cancelToolbarShow = false;
    public saveFooterShow = true;


    public constructor(private readonly breakpointObserver: CommonBreakpointObserver) { }


    public ngOnInit(): void {
        this.menuToolbarShow = this.breakpointObserver.isNarrow && this.menuClick.observers.length > 0;
        this.saveToolbarShow = this.formGroup !== undefined && !this.breakpointObserver.isPointerDevice;
        this.deleteToolbarShow = !this.add && this.deleteClick.observers.length > 0 && !this.breakpointObserver.isPointerDevice;
        this.cancelToolbarShow = (this.breakpointObserver.isNarrow || !this.breakpointObserver.isPointerDevice) && this.cancelClick.observers.length > 0;
        this.saveFooterShow = this.formGroup !== undefined;

        this.breakpointObserver
            .observeNarrow
            .subscribe((state: BreakpointState) => {
                if (state.matches) {
                    this.menuToolbarShow = this.menuClick.observers.length > 0;
                    this.cancelToolbarShow = this.cancelClick.observers.length > 0;
                } else {
                    this.menuToolbarShow = false;
                    this.cancelToolbarShow = !this.breakpointObserver.isPointerDevice && this.cancelClick.observers.length > 0;
                }
            });
    }


    public menu() {
        this.menuClick.emit();
    }


    public delete(): void {
        this.deleteClick.emit();
    }


    public cancel(): void {
        this.cancelClick.emit();
    }


    public get isPointerDevice(): boolean {
        return this.breakpointObserver.isPointerDevice;
    }
}