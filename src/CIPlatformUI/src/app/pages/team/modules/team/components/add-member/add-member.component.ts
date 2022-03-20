import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { InputComponent } from '../../../../../../shared/input/input.component';

@Component({
  selector: 'team-add-member',
  templateUrl: './add-member.component.html',
  styleUrls: ['./add-member.component.scss'],
})
export class AddMemberComponent implements OnInit {
  @ViewChild('username') usernameRef?: InputComponent;
  @Output() onCancel: EventEmitter<any> = new EventEmitter<any>();
  @Output() onAdd: EventEmitter<string> = new EventEmitter<string>();

  constructor() {}

  ngOnInit(): void {}

  cancel(): void {
    this.onCancel.emit();
  }

  add(): void {
    this.onAdd.emit(this.usernameRef!.value);
  }
}
