{{- define "conf.resources" -}}
          {{- if (.requests) }}
          {{- if or (.requests.cpu) (.requests.memory) }}
          requests:
            {{- if (.requests.cpu) }}
            cpu: {{ .requests.cpu }}
            {{- end }}
            {{- if (.requests.memory) }}
            memory: {{ .requests.memory }}
            {{- end }}
          {{- end }}
          {{- end }}
          {{- if (.limits) }}
          {{- if or (.limits.cpu) (.limits.memory) }}
          limits:
            {{- if (.limits.cpu) }}
            cpu: {{ .limits.cpu }}
            {{- end }}
            {{- if (.limits.memory) }}
            memory: {{ .limits.memory }}
            {{- end }}
          {{- end }}
          {{- end }}
{{- end -}}
