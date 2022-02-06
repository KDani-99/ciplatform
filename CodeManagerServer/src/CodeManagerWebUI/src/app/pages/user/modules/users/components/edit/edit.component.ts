import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { UserDto } from '../../../../../../services/user/user.interface';
import { CheckboxComponent } from '../../../../../../shared/checkbox/checkbox.component';

@Component({
  selector: 'user-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditComponent implements OnInit {
  @ViewChild('isAdmin') isAdminRef?: CheckboxComponent;

  @Input() user?: UserDto;
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  constructor() {}

  ngOnInit(): void {
    console.log(this.user, this.isAdminRef);
    if (this.isAdminRef) {
      console.log(this.user);
      this.isAdminRef.isChecked = this.user!.isAdmin;
    }
  }

  close(): void {
    this.onClose.emit();
  }
}
