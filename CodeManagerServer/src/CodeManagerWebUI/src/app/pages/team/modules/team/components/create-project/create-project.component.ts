import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { CreateProjectDto } from '../../../../../project/project.interface';
import { InputComponent } from '../../../../../../shared/input/input.component';
import { CheckboxComponent } from '../../../../../../shared/checkbox/checkbox.component';

@Component({
  selector: 'team-create-project',
  templateUrl: './create-project.component.html',
  styleUrls: ['./create-project.component.scss'],
})
export class CreateProjectComponent implements OnInit {
  @ViewChild('name') nameRef?: InputComponent;
  @ViewChild('description') descriptionRef?: InputComponent;
  @ViewChild('repositoryUrl') repositoryUrlRef?: InputComponent;
  @ViewChild('isPrivateProject') isPrivateProjectRef?: CheckboxComponent;
  @ViewChild('isPrivateRepository') isPrivateRepositoryRef?: CheckboxComponent;
  @ViewChild('username') usernameRef?: InputComponent;
  @ViewChild('secretToken') secretTokenRef?: InputComponent;

  @Input() teamId?: number;
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  @Output() onCreate: EventEmitter<CreateProjectDto> =
    new EventEmitter<CreateProjectDto>();

  constructor() {}

  ngOnInit(): void {}

  close(): void {
    this.onClose.emit();
  }

  create(): void {
    this.onCreate.emit({
      teamId: this.teamId ?? -1,
      name: this.nameRef?.value ?? 'N/A',
      description: this.descriptionRef?.value ?? 'N/A',
      repositoryUrl: this.repositoryUrlRef?.value ?? 'N/A',
      username: this.usernameRef?.value ?? 'N/A',
      secretToken: this.secretTokenRef?.value ?? 'N/A',
      isPrivateProject: this.isPrivateProjectRef?.isChecked ?? false,
      isPrivateRepository: this.isPrivateRepositoryRef?.isChecked ?? false,
    });
  }
}
