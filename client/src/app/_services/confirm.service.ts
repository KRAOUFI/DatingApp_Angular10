import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable, Subscription } from 'rxjs';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  bsModelRef: BsModalRef;

  constructor(private modalService: BsModalService) { }

  confirm(
    message = 'Are you sure you want to do this?',
    title = 'Confirmation',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'): Observable<boolean> {
      const config = {
        initialState: { title, message, btnOkText, btnCancelText }
      };

      this.bsModelRef = this.modalService.show(ConfirmDialogComponent, config);
      return new Observable<boolean>(this.getResult());
  }

  private getResult() {
    return (observe) => {
      const subsciption = this.bsModelRef.onHidden.subscribe(() => {
        observe.next(this.bsModelRef.content.result);
        observe.complete();
      });

      return {
        unsubscribe() {
          subsciption.unsubscribe();
        }
      }
    };
  }
}
