FROM ubuntu:23.10

RUN apt update \
    && apt install -y wget \
    && apt install -y unzip \
    && apt install -y vim \
    && apt install -y openssh-client

RUN wget https://releases.hashicorp.com/terraform/1.6.6/terraform_1.6.6_linux_amd64.zip

RUN unzip terraform_1.6.6_linux_amd64.zip

RUN mv terraform /usr/local/bin/

RUN apt-get install --yes software-properties-common && \
    add-apt-repository --yes --update ppa:ansible/ansible && \
    apt-get install --yes ansible

ENV ANSIBLE_HOST_KEY_CHECKING=False

WORKDIR /src

ENTRYPOINT ["bash"]
