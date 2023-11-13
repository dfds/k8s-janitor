
{{- define "k8s-janitor.serviceaccount.awsRoleArn" -}}
{{- if .Values.serviceAccount.awsRoleArn }}
{{- .Values.serviceAccount.awsRoleArn }}
{{- else }}
{{- "arn:aws:iam::${aws_account_id}:role/k8s-janitor" }}
{{- end }}
{{- end }}