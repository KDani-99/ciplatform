import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  CreateProjectDto,
  ProjectDataDto,
  ProjectDto,
} from '../../../../../../services/project/project.interface';
import { CheckboxComponent } from '../../../../../../shared/checkbox/checkbox.component';
import { InputComponent } from '../../../../../../shared/input/input.component';

@Component({
  selector: 'project-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss'],
})
export class EditComponent implements OnInit {
  @ViewChild('isPrivateProject', { static: true })
  isPrivateProjectRef?: CheckboxComponent;
  @ViewChild('isPrivateRepository', { static: true })
  isPrivateRepositoryRef?: CheckboxComponent;
  @ViewChild('name') nameRef?: InputComponent;
  @ViewChild('description') descriptionRef?: InputComponent;
  @ViewChild('repositoryUrl') repositoryUrlRef?: InputComponent;
  @ViewChild('username') usernameRef?: InputComponent;
  @ViewChild('secretToken') secretTokenRef?: InputComponent;

  @Input() project?: ProjectDataDto;

  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  @Output() onEdit: EventEmitter<CreateProjectDto> =
    new EventEmitter<CreateProjectDto>();

  constructor() {}

  ngOnInit(): void {
    this.isPrivateProjectRef!.isChecked =
      this.project?.project.isPrivateProject ?? false;
    this.isPrivateRepositoryRef!.isChecked =
      this.project?.isPrivateRepository ?? false;
  }

  close(): void {
    this.onClose.emit();
  }

  edit(): void {
    this.onEdit.emit({
      teamId: this.project?.teamId ?? -1,
      name: this.nameRef?.value ?? '',
      description: this.descriptionRef?.value ?? '',
      repositoryUrl: this.repositoryUrlRef?.value ?? '',
      isPrivateProject: this.isPrivateProjectRef?.isChecked ?? false,
      isPrivateRepository: this.isPrivateRepositoryRef?.isChecked ?? false,
      username: this.usernameRef?.value ?? undefined,
      secretToken: this.secretTokenRef?.value ?? undefined,
    });
  }
}
