import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { TeamDto } from '../../../../team.interface';
import { CheckboxComponent } from '../../../../../shared/checkbox/checkbox.component';

@Component({
  selector: 'team-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditComponent implements OnInit {
  @ViewChild('isPrivate', { static: false }) isPrivateRef?: CheckboxComponent;
  @Input() team?: TeamDto;

  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  constructor() {}

  ngOnInit(): void {
    if (this.isPrivateRef && this.team) {
      console.log('asd');
      this.isPrivateRef.isChecked = !this.team.isPublic;
    }
  }

  close(): void {
    this.onClose.emit();
  }
}
