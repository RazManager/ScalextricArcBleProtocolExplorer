import { Component, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';


@Component({
    selector: 'common-static-text',
    templateUrl: './common-static-text.component.html',
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => CommonStaticTextComponent),
            multi: true
        }
    ]
})
export class CommonStaticTextComponent implements ControlValueAccessor {
    @Input() public label!: string;
    @Input() public value: any;

    public writeValue(obj: any): void {
        if (obj) {
            this.value = obj;
        }
    }

    public registerOnChange(fn: any): void {
        // throw new Error("Method not implemented.");
    }

    public registerOnTouched(fn: any): void {
        // throw new Error("Method not implemented.");
    }

    public setDisabledState?(isDisabled: boolean): void {
        // throw new Error("Method not implemented.");
    }
}
