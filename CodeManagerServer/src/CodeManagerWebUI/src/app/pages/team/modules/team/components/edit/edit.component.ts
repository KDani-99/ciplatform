import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  CreateTeamDto,
  TeamDataDto,
} from '../../../../../../services/team/team.interface';
import { CheckboxComponent } from '../../../../../../shared/checkbox/checkbox.component';
import { InputComponent } from '../../../../../../shared/input/input.component';

@Component({
  selector: 'team-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditComponent implements OnInit {
  @ViewChild('isPrivate', { static: true }) isPrivateRef?: CheckboxComponent;
  @ViewChild('name') nameRef?: InputComponent;
  @ViewChild('description') descriptionRef?: InputComponent;
  @ViewChild('image') imageRef?: InputComponent;

  @Input() team?: TeamDataDto;

  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  @Output() onEdit: EventEmitter<CreateTeamDto> =
    new EventEmitter<CreateTeamDto>();
  constructor() {}

  ngOnInit(): void {
    if (this.isPrivateRef && this.team) {
      this.isPrivateRef.isChecked = !this.team.isPublic;
    }
  }

  close(): void {
    this.onClose.emit();
  }

  edit(): void {
    this.onEdit.emit({
      name: this.nameRef?.value ?? '',
      description: this.descriptionRef?.value ?? '',
      image: this.imageRef?.value ?? '',
      isPublic: !this.isPrivateRef?.isChecked,
    });
  }
}
