import { HttpErrorResponse } from '@angular/common/http';

import { environment } from '../../environments/environment';


export abstract class CommonBaseService {
    protected serviceUrl: string;


    constructor(protected readonly servicePartUrl: string) {
        this.serviceUrl = environment.apiBaseUrl + servicePartUrl;
    }


    protected getApiError(err: HttpErrorResponse): ApiErrorResponse {
        const result = new ApiErrorResponse();

        if (err.status === 0) {
            result.title = 'No response from the server.';
            return result;
        }

        const error = <ErrorDto>err.error;
        if (error.errors) {
            if (Object.values(error.errors).length === 0) {
                result.title = error.title;
            }
            else {
                result.title = "";
                Object.values(error.errors).forEach(item => {
                    (<string[]>item).forEach((message: string) => {
                        if (result.title !== "") {
                            result.title += " ";    
                        }
                        result.title += message;
                    });
                });       
            }
        }
        else {
            result.title = err.message;
        }
        return result;
    }
}


export class ApiErrorResponse {
    //public detail: string;
    //public instance: string;
    //public status: number;
    public title!: string;
    //public type: string;
}


class ErrorDto {
    public title!: string;
    public errors: any;
}